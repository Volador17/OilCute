function nd=nndr(x1,x2);

%最邻近距离计算程序；
%x1：矩阵1;
%x2：矩阵2，一般为校正集矩阵；
%nd：矩阵1中的每个点到矩阵2的最邻近距离。


[rx1,cx1] = size(x1);
[rx2,cx2] = size(x2);

ss=inv(x2*x2');
for i=1:cx1
    for j=1:cx2
        d(j)=(x1(:,i)-x2(:,j))'*ss*(x1(:,i)-x2(:,j));
    end
    iii=find(d==0);
    d(iii)=[];
    nd(i)=min(d);
   
end