function md=mahalr(x1,x2);

%马氏距离计算程序;
%x1：矩阵1;
%x2：矩阵2，一般为校正集矩阵；
%计算矩阵1中的每个点到矩阵2的马氏距离。




[rx1,cx1] = size(x1);
[rx2,cx2] = size(x2);

mx2=mean(x2,2);
dx1=x1-mx2(:,ones(cx1,1));
dx2=x2-mx2(:,ones(cx2,1));

ss=inv(dx2*dx2');
for i=1:cx1
    md(i)=dx1(:,i)'*ss*dx1(:,i);
end
