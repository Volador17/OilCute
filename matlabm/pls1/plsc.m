function [w,t,p,b,press,yreg,yp,ye,yte,md,rms,mdt,nndt,my,cvR,mdc,yecv,R,sec,secv]=plsc(x,y,k)
    
%偏最小二乘回归程序PLS1；
%w：权重矢量；
%t：得分；
%p：载荷；
%b：回归系数；
%press：交互验证残差平方和；
%yreg:预测值；
%ye：残差；
%yet:学生残差；
%md:马氏距离；
%rms：光谱残差；
%mdt：马氏距离阈值；
%nndt：最邻近距离阈值；
%my：y的平均值；
%R:预测值与实际值之间的相关系数。
%sec：校正集预测标准偏差；
%secv：交互验证标准误差；
%x：光谱矩阵；
%y：性质向量；
%k：最大主因子数。



[m,n]=size(x);


%交互验证
for i=1:n
    xx=x(:,[1:(i-1), (i+1):n]);
    px=y(:,[1:(i-1), (i+1):n]);
    [w,t,p,b,mmy]=plsc1(xx,px,k);
    ypp=0;
    xy=x(:,i)';
    for j=1:k
        tt(:,j)=xy*w(:,j);
        yp(i,j)=ypp+b(j)*tt(:,j)+mmy;
        ypp=ypp+b(j)*tt(:,j);
        xy=xy-tt(:,j)*p(:,j)';
    end
    tc(i,:)=tt;
end

 %---------------------------------------------------
    tc=tc';
for i=1:k
    press(i)=(y-yp(:,i)')*(y-yp(:,i)')';
    secv(i)=sqrt(press(i)/n);


    %计算残差
    yecv(i,:)=yp(:,i)'-y;
 %计算相关系数；
cvR(i)=(sum((yp(:,i)'-mmy).*(yp(:,i)'-mmy)))/(sum(yecv(i,:).*yecv(i,:))+sum((yp(:,i)'-mmy).*(yp(:,i)'-mmy)));
   
    %计算马氏距离

    mdc(i,:)=mahalr(tc(1:i,:),tc(1:i,:));
 %------------------------------------------------------
end

[w,t,p,b,my]=plsc1(x,y,k);%计算偏最小二乘回归参数
t=t';
p=p';
ypp=0;
x=x';
 for i=1:k
    %计算预测值
    yreg(i,:)=(ypp+b(i)*t(i,:)'+my)';
    ypp=ypp+b(i)*t(i,:)';
    x=x-t(i,:)'*p(i,:);

        %计算残差
    ye(i,:)=yreg(i,:)-y;
    sec(i)=sqrt(ye(i,:)*ye(i,:)'/(n-1));%计算预测标准偏差

    
    %计算相关系数；
    R(i)=(sum((yreg(i,:)-mmy).*(yreg(i,:)-mmy)))/(sum(ye(i,:).*ye(i,:))+sum((yreg(i,:)-mmy).*(yreg(i,:)-mmy)));



    %计算马氏距离及阈值
    md(i,:)=mahalr(t(1:i,:),t(1:i,:));
    mdt(i)=max(md(i,:));

    
    %计算学生残差
    yte(i,:)=abs(ye(i,:)./(sec(i)*sqrt(1-md(i,:))));
    
    %计算最邻近距离
    nd(i,:)=nndr(t(1:i,:),t(1:i,:));
    nndt(i)=max(nd(i,:));
    
    %计算光谱残差
    rms(i,:)=rmssr(x',p(1:i,:));
    end

     yp=yp';