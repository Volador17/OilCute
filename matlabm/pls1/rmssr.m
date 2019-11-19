function rms=rmssr(x,l);

%光谱残差计算程序：
%x：光谱矩阵或光谱向量；
%l：光谱主成分分析载荷；
%rmssr：光谱残差。



[m,n]=size(x);
r=x-l'*(l*x);
for i=1:n
    rms(i)=sqrt(r(:,i)'*r(:,i)/m);
end